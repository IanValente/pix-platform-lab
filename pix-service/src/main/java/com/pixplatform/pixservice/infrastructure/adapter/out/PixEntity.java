package com.pixplatform.pixservice.infrastructure.adapter.out;

import jakarta.persistence.*;
import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.util.UUID;

@Entity
@Table(name = "pix_transactions")
public class PixEntity {

    @Id
    private UUID id;
    private String pixKey; // 'key' é palavra reservada em alguns bancos, melhor evitar
    private BigDecimal amount;
    private String status;
    private LocalDateTime createdAt;

    // Construtor vazio obrigatório para o JPA
    public PixEntity() {}

    // Construtor para facilitar o mapeamento
    public PixEntity(UUID id, String pixKey, BigDecimal amount, String status, LocalDateTime createdAt) {
        this.id = id;
        this.pixKey = pixKey;
        this.amount = amount;
        this.status = status;
        this.createdAt = createdAt;
    }

    public UUID getId() {
        return id;
    }

    public void setId(UUID id) {
        this.id = id;
    }

    public String getPixKey() {
        return pixKey;
    }

    public void setPixKey(String pixKey) {
        this.pixKey = pixKey;
    }

    public BigDecimal getAmount() {
        return amount;
    }

    public void setAmount(BigDecimal amount) {
        this.amount = amount;
    }

    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }

    public LocalDateTime getCreatedAt() {
        return createdAt;
    }

    public void setCreatedAt(LocalDateTime createdAt) {
        this.createdAt = createdAt;
    }
}