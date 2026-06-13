package com.pixplatform.pixservice.domain.model;

import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import java.math.BigDecimal;
import static org.junit.jupiter.api.Assertions.*;

class PixTest {

    @Test
    @DisplayName("Should create a new Pix with CREATED status and valid UUID")
    void shouldCreateNewPix() {
        // Arrange (Preparar)
        String key = "teste@teste.com";
        BigDecimal amount = new BigDecimal("100.50");

        // Act (Agir)
        Pix pix = new Pix(key, amount);

        // Assert (Verificar)
        assertNotNull(pix.getId());
        assertEquals(key, pix.getKey());
        assertEquals(amount, pix.getAmount());
        assertEquals(PixStatus.CREATED, pix.getStatus());
        assertNotNull(pix.getCreatedAt());
    }

    @Test
    @DisplayName("Should complete a Pix when status is CREATED")
    void shouldCompletePix() {
        // Arrange
        Pix pix = new Pix("teste@teste.com", BigDecimal.TEN);

        // Act
        pix.complete();

        // Assert
        assertEquals(PixStatus.COMPLETED, pix.getStatus());
    }

    @Test
    @DisplayName("Should throw exception when trying to complete an already completed Pix")
    void shouldThrowExceptionWhenCompletingAlreadyCompletedPix() {
        // Arrange
        Pix pix = new Pix("teste@teste.com", BigDecimal.TEN);
        pix.complete(); // Completa a primeira vez com sucesso

        // Act & Assert
        IllegalStateException exception = assertThrows(IllegalStateException.class, pix::complete);
        assertEquals("Apenas um Pix CREATED pode ser completado", exception.getMessage());
    }
}