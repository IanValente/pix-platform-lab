package com.pixplatform.pixservice.infrastructure.config;

import com.pixplatform.pixservice.application.port.out.SavePixPort;
import com.pixplatform.pixservice.application.port.out.SendPixCreatedEventPort;
import com.pixplatform.pixservice.application.usecase.CreatePixService;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
public class PixConfig {

    @Bean
    public CreatePixService createPixUseCase(SavePixPort savePixPort, SendPixCreatedEventPort sendEventPort) {
        return new CreatePixService(savePixPort, sendEventPort);
    }
}