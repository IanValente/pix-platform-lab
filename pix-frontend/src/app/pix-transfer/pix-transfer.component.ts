import { Component, DestroyRef, inject, signal } from '@angular/core';
import { PixService } from '../pix.service';
import { FormsModule } from '@angular/forms'; // Necessário para pegar dados do input HTML
import { interval, switchMap, takeWhile, catchError, of } from 'rxjs'; // OS PODERES DO RXJS
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-pix-transfer', // O nome da "tag" HTML que esse componente vai gerar
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './pix-transfer.component.html',
  styleUrl: './pix-transfer.component.css'
})
export class PixTransferComponent {
  private readonly destroyRef = inject(DestroyRef);

  // Variáveis que vão se conectar com o HTML via Signals
  chavePix = signal('');
  valor = signal(0);
  mensagem = signal('');

  // Variáveis para a nova sessão de consulta
  consultaId = signal('');
  statusRetornado = signal<any | null>(null);

  // Injetando nosso Service (Inversão de Controle)
  constructor(private pixService: PixService) {}

  enviar() {
    this.mensagem.set('Enviando...');
    this.statusRetornado.set(null); // Limpa a tela se for um novo Pix
    
    // .subscribe() é o gatilho que diz: "Pode executar o HTTP agora, e me avise quando voltar"
    this.pixService.createPix(this.chavePix(), this.valor()).subscribe({
      next: (resposta : any) => { // Equivalente ao HTTP 200 OK
        this.mensagem.set('Pix enviado com sucesso pro Java!');
        this.consultaId.set(resposta.transactionId);
        this.chavePix.set(''); // Limpa o formulário
        this.valor.set(0);

        // Em vez de só dar sucesso, iniciamos a perseguição ao C#!
        this.iniciarPolling(this.consultaId());
      },
      error: (erro) => { // Equivalente ao Catch de uma Exception
        const mensagemBackend =
          erro?.error?.mensagem ||
          (typeof erro.error === 'string' ? erro.error : null);

        if (mensagemBackend) {
          this.mensagem.set(`Aviso: ${mensagemBackend}`);
        } else {
          this.mensagem.set('Erro ao conectar com o servidor.');
        }
        console.error(erro);
      }
    });
  }

  iniciarPolling(id: string) {
    this.mensagem.set('Processando liquidação em background... 🔄');

    // 1. Inicia o relógio a cada 2 segundos (2000 ms)
    interval(2000).pipe(
      // 2. Troca o pulso do relógio por uma chamada HTTP ao C#
      switchMap(() => this.pixService.checkStatus(id).pipe(
        // Se o C# der 404, o catchError intercepta, engole o erro e retorna 'null'.
        // Isso impede que o RxJS exploda e destrua o nosso relógio.
        catchError(() => of(null)) 
      )),
      // 3. Continua o relógio ENQUANTO o retorno for 'null'. 
      // O 'true' final significa "emita o valor que quebrou a regra (o JSON com sucesso) antes de parar"
      takeWhile((dados) => dados === null, true),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (dadosDaLiquidacao) => {
        if (dadosDaLiquidacao !== null) {
          // Quando finalmente não for null, o C# encontrou!
          this.statusRetornado.set(dadosDaLiquidacao);
          this.mensagem.set('Pix Liquidado com Sucesso! ✅');
        }
      }
    });
  }

  // NOVO MÉTODO
  consultar() {
    this.pixService.checkStatus(this.consultaId()).subscribe({
      next: (dados) => {
        this.statusRetornado.set(dados);
      },
      error: (erro) => {
        this.statusRetornado.set({ status: 'Não encontrado ou erro na rede' });
      }
    });
  }
}