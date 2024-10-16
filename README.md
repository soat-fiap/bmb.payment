## Overview

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=soat-fiap_bmb.payment&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=soat-fiap_bmb.payment)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=soat-fiap_bmb.payment&metric=coverage)](https://sonarcloud.io/summary/new_code?id=soat-fiap_bmb.payment)

This project, `bmb.payment`, is the payment microservice for a fast food application. It provides a simple API that allows users to create payments and integrates with various payment service providers to handle payment creation and receive payment status notifications. Additionally, it integrates with other domain microservices through messaging using MassTransit.

## Features

- **Payment Creation**: Easily create payments through a simple API.
- **Payment Integration**: Integrates with multiple payment service providers.
- **Payment Notifications**: Receives and processes payment status notifications.
- **Microservice Communication**: Uses MassTransit for messaging between domain microservices.

## Getting Started

### Prerequisites

- [.NET Core](https://dotnet.microsoft.com/download)
- [MassTransit](https://masstransit-project.com/)
- [Docker](https://www.docker.com/) (optional, for containerization)

### Installation

1. Clone the repository:
    ```sh
    git clone https://github.com/soat-fiap/bmb.payment.git
    ```
2. Navigate to the project directory:
    ```sh
    cd bmb.payment
    ```
3. Restore dependencies:
    ```sh
    dotnet restore
    ```

### Running the Application

To run the application locally, use the following command:
```sh
dotnet run
```

### Docker Support

To run the application in a Docker container:
1. Build the Docker image:
    ```sh
    docker build -t bmb.payment .
    ```
2. Run the Docker container:
    ```sh
    docker run -d -p 5000:80 bmb.payment
    ```

## Usage

### API Endpoints

- **Create Payment**: `POST /api/payments`
- **Get Payment Status**: `GET /api/payments/{id}/status`

### Example Request

```sh
curl -X POST "https://yourapiurl/api/payments" -H "accept: application/json" -H "Content-Type: application/json" -d "{ \"amount\": 100.0, \"currency\": \"USD\", \"paymentMethod\": \"CreditCard\" }"
```