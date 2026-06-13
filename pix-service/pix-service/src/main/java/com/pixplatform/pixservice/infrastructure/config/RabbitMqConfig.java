package com.pixplatform.pixservice.infrastructure.config;

import org.springframework.amqp.core.*;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
public class RabbitMqConfig {

    public static final String EXCHANGE = "pix.exchange";
    public static final String QUEUE = "pix.created.queue";
    public static final String ROUTING_KEY = "pix.created.routingKey";

    @Bean
    public Queue pixCreatedQueue() {
        return new Queue(QUEUE, true);
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