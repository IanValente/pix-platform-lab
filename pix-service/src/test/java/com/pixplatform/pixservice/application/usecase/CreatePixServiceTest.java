package com.pixplatform.pixservice.application.usecase;

import com.pixplatform.pixservice.application.port.out.SavePixPort;
import com.pixplatform.pixservice.application.port.out.SendPixCreatedEventPort; // 1. Novo import adicionado
import com.pixplatform.pixservice.domain.model.Pix;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.mockito.Mockito;

import java.math.BigDecimal;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.*;

class CreatePixServiceTest {

    private SavePixPort savePixPortMock;
    private SendPixCreatedEventPort sendPixCreatedEventPortMock; // 2. Variável da segunda porta
    private CreatePixService createPixService;

    @BeforeEach
    void setUp() {
        // Cria os dublês falsos
        savePixPortMock = Mockito.mock(SavePixPort.class);
        sendPixCreatedEventPortMock = Mockito.mock(SendPixCreatedEventPort.class); // 3. Inicializa o dublê

        // 4. Injeta as DUAS dependências no serviço real para a compilação passar
        createPixService = new CreatePixService(savePixPortMock, sendPixCreatedEventPortMock);
    }

    @Test
    @DisplayName("Should orchestrate the creation and saving of a Pix")
    void shouldExecutePixCreation() {
        // Arrange
        String key = "telefone-11999999999";
        BigDecimal amount = new BigDecimal("50.00");

        // Ensinando o dublê: "Quando pedirem para você salvar qualquer Pix, devolva o próprio Pix"
        when(savePixPortMock.save(any(Pix.class))).thenAnswer(invocation -> invocation.getArgument(0));

        // Act
        Pix savedPix = createPixService.execute(key, amount);

        // Assert
        assertNotNull(savedPix);
        assertEquals(key, savedPix.getKey());

        // Verifica se o nosso serviço realmente chamou a porta de salvar exatamente 1 vez
        verify(savePixPortMock, times(1)).save(any(Pix.class));
    }
}