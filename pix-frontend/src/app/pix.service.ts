import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root' // Isso é exatamente o @Service do Spring. Cria um Singleton global.
})
export class PixService {
  // Equivalente ao RestTemplate do Java
  constructor(private http: HttpClient) { }

  createPix(key: string, amount: number) {
    const payload = { key: key, amount: amount }; // Monta o JSON
    
    // O post() do Angular não dispara na hora. Ele retorna um 'Observable' (uma promessa de entrega).
    return this.http.post('http://localhost:8080/api/v1/pix', payload);
  }

  checkStatus(id: string) {
    return this.http.get(`http://localhost:5193/api/v1/settlement/${id}`);
  }
}