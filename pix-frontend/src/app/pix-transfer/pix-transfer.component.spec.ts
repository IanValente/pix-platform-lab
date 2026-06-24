import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { of, throwError } from 'rxjs';
import { PixService } from '../pix.service';
import { PixTransferComponent } from './pix-transfer.component';

describe('PixTransferComponent', () => {
  let fixture: ComponentFixture<PixTransferComponent>;
  let component: PixTransferComponent;
  let pixServiceMock: PixService;

  beforeEach(async () => {
    // 1. Ligamos a máquina do tempo do Vitest!
    vi.useFakeTimers();

    pixServiceMock = {
      createPix: vi.fn(),
      checkStatus: vi.fn()
    } as unknown as PixService;

    await TestBed.configureTestingModule({
      imports: [PixTransferComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: PixService, useValue: pixServiceMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(PixTransferComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  afterEach(() => {
    vi.restoreAllMocks();
    // 2. Desligamos a máquina do tempo e devolvemos ao normal
    vi.useRealTimers();
  });

  // 3. O teste agora é apenas "async ()", sem fakeAsync
  it('deve criar a transação com sucesso, atualizar os Signals e não vazar memória', async () => {
    const transactionId = '65a1ff20-abcb-4799-833d-76114e2d2615';
    const pixResponse = { transactionId, status: 'CREATED' };
    const settlementResponse = {
      status: 'COMPLETED',
      amount: 100,
      processedAt: '2026-06-23T00:00:00'
    };

    (pixServiceMock.createPix as ReturnType<typeof vi.fn>).mockReturnValue(of(pixResponse));
    (pixServiceMock.checkStatus as ReturnType<typeof vi.fn>).mockReturnValue(of(settlementResponse));

    component.chavePix.set('teste@pix');
    component.valor.set(100);

    component.enviar();

    expect(component.mensagem()).toBe('Processando liquidação em background... 🔄');
    expect(component.consultaId()).toBe(transactionId);
    expect(component.chavePix()).toBe('');
    expect(component.valor()).toBe(0);

    // 4. Avançamos 2 segundos no tempo usando a ferramenta nativa do Vitest
    await vi.advanceTimersByTimeAsync(2000);

    expect(pixServiceMock.checkStatus).toHaveBeenCalledTimes(1);
    expect(component.statusRetornado()?.status).toBe('COMPLETED');
    expect(component.mensagem()).toBe('Pix Liquidado com Sucesso! ✅');

    fixture.destroy();

    // 5. Avançamos mais 4 segundos para provar que o polling morreu (Memory Leak prevention)
    await vi.advanceTimersByTimeAsync(4000);

    expect(pixServiceMock.checkStatus).toHaveBeenCalledTimes(1);
  });

  it('deve interceptar erro 400 em formato String e atualizar o Signal da mensagem com o aviso', () => {
    (pixServiceMock.createPix as ReturnType<typeof vi.fn>).mockReturnValue(
      throwError(() => ({
        status: 400,
        error: { mensagem: 'O valor do Pix deve ser maior que zero.' }
      }))
    );

    component.enviar();

    expect(component.mensagem()).toBe('Aviso: O valor do Pix deve ser maior que zero.');
    expect(component.statusRetornado()).toBeNull();
  });

  it('deve interceptar erro de servidor genérico e exibir mensagem de falha', () => {
    (pixServiceMock.createPix as ReturnType<typeof vi.fn>).mockReturnValue(
      throwError(() => ({
        status: 500,
        error: { message: 'Internal Server Error' }
      }))
    );

    component.enviar();

    expect(component.mensagem()).toBe('Erro ao conectar com o servidor.');
    expect(component.statusRetornado()).toBeNull();
  });
});