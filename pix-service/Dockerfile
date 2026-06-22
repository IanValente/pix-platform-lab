# ESTÁGIO 1: Construção (A Fábrica Pesada)
# Usamos uma imagem que já tem o Maven e o JDK instalados
FROM maven:3.9-eclipse-temurin-21-alpine AS build
WORKDIR /app

# Primeiro, copiamos apenas o pom.xml e baixamos as dependências.
# Isso usa o cache do Docker para não baixar a internet inteira toda vez que você mudar o código.
COPY pom.xml .
RUN mvn dependency:go-offline

# Depois, copiamos o código fonte e geramos o .jar ignorando os testes (que já rodam no CI)
COPY src ./src
RUN mvn clean package -DskipTests

# ESTÁGIO 2: Execução (A Loja Leve)
# Usamos uma imagem super pequena que só tem o JRE (Java Runtime Environment)
FROM eclipse-temurin:21-jre-alpine
WORKDIR /app

# Pegamos APENAS o .jar gerado no Estágio 1 e ignoramos o Maven e o código fonte
COPY --from=build /app/target/*.jar app.jar

# Avisamos qual porta o Spring Boot vai usar
EXPOSE 8080

# Ligamos o motor
ENTRYPOINT ["java", "-jar", "app.jar"]