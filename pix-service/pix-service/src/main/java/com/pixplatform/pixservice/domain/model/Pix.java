package com.pixplatform.pixservice.domain.model;

import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.util.UUID;

public class Pix {

    private UUID id;
    private String key;
    private BigDecimal amount;
    private PixStatus status;
    private LocalDateTime createdAt;

    public Pix(String key, BigDecimal amount) {
        this.id = UUID.randomUUID();
        this.key = key;
        this.amount = amount;
        this.status = PixStatus.CREATED;
        this.createdAt = LocalDateTime.now();
    }

    public UUID getId() {
        return id;
    }

    public void setId(UUID id) {
        this.id = id;
    }

    public String getKey() {
        return key;
    }

    public void setKey(String key) {
        this.key = key;
    }

    public BigDecimal getAmount() {
        return amount;
    }

    public void setAmount(BigDecimal amount) {
        this.amount = amount;
    }

    public PixStatus getStatus() {
        return status;
    }

    public void setStatus(PixStatus status) {
        this.status = status;
    }

    public LocalDateTime getCreatedAt() {
        return createdAt;
    }

    public void setCreatedAt(LocalDateTime createdAt) {
        this.createdAt = createdAt;
    }

    public void complete() {
        if (this.status != PixStatus.CREATED) {
            throw new IllegalStateException("Apenas um Pix CREATED pode ser completado");
        }
        this.status = PixStatus.COMPLETED;
    }
}