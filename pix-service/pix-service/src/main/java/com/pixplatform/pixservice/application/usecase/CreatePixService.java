package com.pixplatform.pixservice.application.usecase;

import com.pixplatform.pixservice.application.port.in.CreatePixUseCase;
import com.pixplatform.pixservice.application.port.out.SavePixPort;
import com.pixplatform.pixservice.domain.model.Pix;
import java.math.BigDecimal;

public class CreatePixService implements CreatePixUseCase {

    private final SavePixPort savePixPort;

    // Injeção de dependência via construtor (Sempre prefira isso ao invés de @Autowired)
    public CreatePixService(SavePixPort savePixPort) {
        this.savePixPort = savePixPort;
    }

    @Override
    public Pix execute(String key, BigDecimal amount) {
        // 1. Instancia o objeto de domínio (regra de negócio pura)
        Pix pix = new Pix(key, amount);

        // 2. Manda salvar através da porta de saída
        return savePixPort.save(pix);
    }
}