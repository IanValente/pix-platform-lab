# Pix Platform Lab

Laboratório de engenharia de software e arquitetura distribuída focado no desenho e implementação de um ecossistema simplificado de transações Pix.

## 🎯 Objetivo do Projeto
Este repositório não visa a construção de um produto comercial, mas sim um ambiente controlado para estudo prático de microsserviços, mensageria de alta performance, resiliência e observabilidade em sistemas financeiros.

## 🏗️ Arquitetura Proposta (Visão Geral)
O projeto será composto por múltiplos serviços integrados de forma assíncrona:
- **Pix Service (Java / Spring Boot):** Core de processamento de chaves e transações.
- **Settlement Service (.NET 8):** Serviço poliglota para conciliação bancária financeira.
- **Notification Service (Node.js / TypeScript):** Disparo de eventos e alertas orientados a eventos.
- **Frontend (Angular):** Interface de acompanhamento das operações em tempo real.

## 🚀 Roadmap de Desenvolvimento
- [x] Fase 0: Inicialização do Repositório e Alinhamento Técnico
- [ ] Fase 1: Setup do Ambiente Local (Docker, Postgres, Ferramental)
- [ ] Fase 2: Construção do Core Pix Service (Java)
- [ ] Fase 3: Introdução de Mensageria (RabbitMQ)
- [ ] Fase 4: Integração do Serviço de Liquidação (.NET 8)
- [ ] Fase 5: Notificações, Frontend e Observabilidade

---
Desenvolvido para fins de estudo e evolução técnica.