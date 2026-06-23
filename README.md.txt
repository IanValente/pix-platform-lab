# 🚀 Pix Platform Lab

![Angular](https://img.shields.io/badge/Angular-DD0031?style=for-the-badge&logo=angular&logoColor=white)
![Spring Boot](https://img.shields.io/badge/Spring_Boot-6DB33F?style=for-the-badge&logo=spring&logoColor=white)
![.NET](https://img.shields.io/badge/.NET_8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-FF6600?style=for-the-badge&logo=rabbitmq&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)

## 📌 Resumo do Projeto
Laboratório prático de arquitetura distribuída simulando um ecossistema de pagamentos (Pix). O projeto integra múltiplos microsserviços heterogêneos, aplicando padrões modernos de engenharia de software para garantir escalabilidade, resiliência e comunicação assíncrona.

## 🏗️ Decisões Arquiteturais de Destaque
* **Arquitetura Orientada a Eventos (EDA):** Desacoplamento entre a API de geração de Pix (Java) e a API de liquidação (C#) utilizando RabbitMQ. Isso garante que a aplicação não caia caso o banco de liquidação sofra instabilidades.
* **Resiliência e Self-Healing:** Infraestrutura orquestrada via Docker Compose com políticas de `restart: on-failure`. As APIs estão configuradas para suportar concorrência e indisponibilidade temporária de rede (ex: banco de dados iniciando).
* **Containerização Otimizada:** Uso de *Multi-stage Builds* nos Dockerfiles para garantir imagens de produção leves, imutáveis e seguras, separando o SDK de build do runtime de execução.
* **Front-end Reativo Nativo:** Migração de RxJS complexo para **Angular Signals**, eliminando riscos de vazamento de memória (*Memory Leaks*) e garantindo alta performance na renderização de estados.

## 🔄 Fluxo de Dados (Event-Driven)
1. 👤 O usuário inicia uma transferência no front-end **Angular**.
2. ☕ A requisição é processada pela API produtora em **Java/Spring Boot** (`POST /api/v1/pix`).
3. 🐘 O serviço Java persiste a intenção de pagamento no **PostgreSQL** e publica um evento no **RabbitMQ**.
4. ⚙️ O consumidor em **C# / .NET 8** escuta a fila, apanha o evento e processa a liquidação da transação.
5. 🗄️ O resultado é gravado com Entity Framework no **SQL Server** através de *Auto-Migrations*.
6. ⚡ O front-end consulta assincronamente a liquidação em background e atualiza a tela de forma reativa.

## 🛠️ Stack Tecnológica
| Camada | Tecnologia | Propósito |
|---|---|---|
| **Front-end UI** | Angular 17+ (Signals) | Interface do usuário e reatividade de estado |
| **Back-end Produtor**| Java 21 + Spring Boot | Validação e criação da intenção de pagamento |
| **Banco de Dados 1** | PostgreSQL | Persistência do serviço produtor |
| **Mensageria** | RabbitMQ | *Message Broker* para integração assíncrona |
| **Back-end Consumidor**| C# + .NET 8 | Processamento e liquidação de regras de negócio |
| **Banco de Dados 2** | SQL Server | Persistência do serviço consumidor |
| **Infra/DevOps** | Docker + Compose | Orquestração e conteinerização isolada |

## 🚀 Como Executar
O projeto foi desenhado para rodar com um único comando, construindo sua própria infraestrutura e bancos de dados do zero.

1. Clone o repositório e abra o terminal na raiz do projeto.
2. Execute o comando de orquestração:
```bash
docker-compose up -d --build

Aguarde o provisionamento (o C# criará o schema do banco automaticamente).

Acesse o Front-end em: http://localhost:4200