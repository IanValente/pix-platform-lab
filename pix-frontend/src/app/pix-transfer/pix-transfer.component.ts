import { ChangeDetectorRef, Component } from '@angular/core';
import { PixService } from '../pix.service';
import { FormsModule } from '@angular/forms'; // Necessário para pegar dados do input HTML
import { CommonModule } from '@angular/common'; // Necessário para o *ngIf no Standalone Component
import { interval, switchMap, takeWhile, catchError, of } from 'rxjs'; // OS PODERES DO RXJS

@Component({
  selector: 'app-pix-transfer', // O nome da "tag" HTML que esse componente vai gerar
  standalone: true,
  imports: [FormsModule],
  templateUrl: './pix-transfer.component.html',
  styleUrl: './pix-transfer.component.css'
})
export class PixTransferComponent {
  // Variáveis que vão se conectar com o HTML
  chavePix: string = '';
  valor: number = 0;
  mensagem: string = '';

  // Variáveis para a nova sessão de consulta
  consultaId: string = '';
  statusRetornado: any = null; // 'any' permite receber o JSON dinâmico do C#

  // Injetando nosso Service (Inversão de Controle)
  constructor(private pixService: PixService,
              private cdr: ChangeDetectorRef
  ) {}

  enviar() {
    this.mensagem = 'Enviando...';
    this.statusRetornado = null; // Limpa a tela se for um novo Pix
    
    // .subscribe() é o gatilho que diz: "Pode executar o HTTP agora, e me avise quando voltar"
    this.pixService.createPix(this.chavePix, this.valor).subscribe({
      next: (resposta : any) => { // Equivalente ao HTTP 200 OK
        this.mensagem = 'Pix enviado com sucesso pro Java!';
        this.consultaId = resposta.transactionId;
        this.chavePix = ''; // Limpa o formulário
        this.valor = 0;

        // Em vez de só dar sucesso, iniciamos a perseguição ao C#!
        this.iniciarPolling(this.consultaId);
      },
      error: (erro) => { // Equivalente ao Catch de uma Exception
        this.mensagem = 'Erro ao conectar com o servidor.';
        console.error(erro);

        this.cdr.detectChanges();
      }
    });
  }

  iniciarPolling(id: string) {
    this.mensagem = 'Processando liquidação em background... 🔄';
    this.cdr.detectChanges();

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
      takeWhile((dados) => dados === null, true) 
    ).subscribe({
      next: (dadosDaLiquidacao) => {
        if (dadosDaLiquidacao !== null) {
          // Quando finalmente não for null, o C# encontrou!
          this.statusRetornado = dadosDaLiquidacao;
          this.mensagem = 'Pix Liquidado com Sucesso! ✅';
          this.cdr.detectChanges(); // Atualiza a tela
        }
      }
    });
  }

  // NOVO MÉTODO
  consultar() {
    this.pixService.checkStatus(this.consultaId).subscribe({
      next: (dados) => {
        this.statusRetornado = dados;
        this.cdr.detectChanges(); // O pulo do gato para a tela atualizar
      },
      error: (erro) => {
        this.statusRetornado = { status: 'Não encontrado ou erro na rede' };
        this.cdr.detectChanges();
      }
    });
  }
}