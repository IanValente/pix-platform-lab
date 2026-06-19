import { Component } from '@angular/core';
// 1. O Import da Classe (Igual ao import do Java)
import { PixTransferComponent } from './pix-transfer/pix-transfer.component'; 

@Component({
  selector: 'app-root',
  standalone: true,
  // 2. A Injeção no Decorator: Apresenta o componente filho para o componente pai
  imports: [PixTransferComponent], 
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'pix-frontend';
}