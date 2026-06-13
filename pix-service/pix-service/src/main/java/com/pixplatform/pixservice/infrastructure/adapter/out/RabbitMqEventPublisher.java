package com.pixplatform.pixservice.infrastructure.adapter.out;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.pixplatform.pixservice.application.port.out.SendPixCreatedEventPort;
import com.pixplatform.pixservice.domain.model.Pix;
import org.springframework.amqp.rabbit.core.RabbitTemplate;
import org.springframework.stereotype.Component;

@Component
public class RabbitMqEventPublisher implements SendPixCreatedEventPort {

    private final RabbitTemplate rabbitTemplate;
    // Criamos o nosso próprio motor de JSON, blindando contra as injeções do Spring
    private final ObjectMapper objectMapper = new ObjectMapper();

    private static final String EXCHANGE = "pix.exchange";
    private static final String ROUTING_KEY = "pix.created.routingKey";

    // O Spring agora só precisa injetar o RabbitTemplate (que ele já conhece)
    public RabbitMqEventPublisher(RabbitTemplate rabbitTemplate) {
        this.rabbitTemplate = rabbitTemplate;
    }

    @Override
    public void send(Pix pix) {
        try {
            PixCreatedEvent event = new PixCreatedEvent(pix.getId().toString(), pix.getKey(), pix.getAmount());

            String jsonPayload = objectMapper.writeValueAsString(event);

            rabbitTemplate.convertAndSend(EXCHANGE, ROUTING_KEY, jsonPayload);

            System.out.println("Evento publicado no RabbitMQ: " + jsonPayload);

        } catch (Exception e) {
            throw new RuntimeException("Falha ao converter evento do Pix para JSON", e);
        }
    }
}

record PixCreatedEvent(String id, String key, java.math.BigDecimal amount) {}