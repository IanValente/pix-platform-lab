package com.pixplatform.pixservice.infrastructure.adapter.out;

import com.pixplatform.pixservice.application.port.out.SavePixPort;
import com.pixplatform.pixservice.domain.model.Pix;
import com.pixplatform.pixservice.domain.model.PixStatus;
import org.springframework.stereotype.Component;

@Component // Aqui o Spring entra, pois estamos na camada de Infraestrutura
public class PixDatabaseAdapter implements SavePixPort {

    private final PixJpaRepository repository;

    public PixDatabaseAdapter(PixJpaRepository repository) {
        this.repository = repository;
    }

    @Override
    public Pix save(Pix pix) {
        // 1. Traduz do Domínio para a Entidade do Banco
        PixEntity entity = new PixEntity(
                pix.getId(),
                pix.getKey(),
                pix.getAmount(),
                pix.getStatus().name(),
                pix.getCreatedAt()
        );

        // 2. Salva no banco de dados via Spring Data
        PixEntity savedEntity = repository.save(entity);

        // 3. (Simplificação) Retornamos o próprio objeto de domínio que recebemos
        // Em um cenário real, traduziríamos a 'savedEntity' de volta para 'Pix' caso o banco gerasse algum dado novo.
        return pix;
    }
}