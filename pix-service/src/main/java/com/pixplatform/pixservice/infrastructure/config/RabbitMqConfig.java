package com.pixplatform.pixservice.infrastructure.config;

import org.springframework.amqp.core.*;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
public class RabbitMqConfig {

    public static final String EXCHANGE = "pix.exchange";
    public static final String QUEUE = "pix.created.queue";
    public static final String ROUTING_KEY = "pix.created.routingKey";

    // NOVO: Declaramos o nome da fila do cemitério (DLQ)
    public static final String DLQ = "pix.created.dlq";

    // NOVO: Avisamos o Java que a DLQ existe
    @Bean
    public Queue dlq() {
        return QueueBuilder.durable(DLQ).build();
    }

    // ATUALIZADO: A fila principal agora usa o QueueBuilder e tem as exatas mesmas regras do C#
    @Bean
    public Queue pixCreatedQueue() {
        return QueueBuilder.durable(QUEUE)
                .withArgument("x-dead-letter-exchange", "")
                .withArgument("x-dead-letter-routing-key", DLQ)
                .build();
    }

    @Bean
    public DirectExchange pixExchange() {
        return new DirectExchange(EXCHANGE);
    }

    @Bean
    public Binding binding(Queue pixCreatedQueue, DirectExchange pixExchange) {
        return BindingBuilder.bind(pixCreatedQueue).to(pixExchange).with(ROUTING_KEY);
    }
}