package com.pixplatform.pixservice.application.usecase;

import com.pixplatform.pixservice.application.port.in.CreatePixUseCase;
import com.pixplatform.pixservice.application.port.out.SavePixPort;
import com.pixplatform.pixservice.application.port.out.SendPixCreatedEventPort;
import com.pixplatform.pixservice.domain.model.Pix;
import java.math.BigDecimal;

public class CreatePixService implements CreatePixUseCase {

    private final SavePixPort savePixPort;
    private final SendPixCreatedEventPort sendEventPort; // Nossa nova porta

    // Injeção de dependência via construtor (Sempre prefira isso ao invés de @Autowired)
    public CreatePixService(SavePixPort savePixPort, SendPixCreatedEventPort sendEventPort) {
        this.savePixPort = savePixPort;
        this.sendEventPort = sendEventPort;
    }

    @Override
    public Pix execute(String key, BigDecimal amount) {
        if (amount == null || amount.compareTo(BigDecimal.ZERO) <= 0) {
            throw new IllegalArgumentException("O valor do Pix deve ser maior que zero.");
        }
        Pix pix = new Pix(key, amount);
        Pix savedPix = savePixPort.save(pix);

        // Dispara o evento de forma assíncrona desacoplada
        sendEventPort.send(savedPix);

        return savedPix;
    }
}