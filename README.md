# Care Calendar - Full-Stack App

![core](https://github.com/stokes-adam/care-calendar/actions/workflows/infra-core.yaml/badge.svg)
![backend](https://github.com/stokes-adam/care-calendar/actions/workflows/backend-infra.yaml/badge.svg)
![frontend](https://github.com/stokes-adam/care-calendar/actions/workflows/frontend-infra.yaml/badge.svg)


## Introduction

`Care Calendar` is a full-fledged booking and management solution, designed to assist receptionists in efficiently managing specialists, rooms, and client appointments. This system ensures that there are no overlapping schedules for clients, specialists, and room availabilities.

## Repository Structure

The repository is structured to be expandable, with the current focus on the booking tool. This tool, which is located in its own folder, is designed with the potential for future expansions, allowing for integration of multiple solutions.

### Booking

Booking serves as the core functionality of this solution, allowing receptionists to:

- Schedule appointments.
- Avoid overlapping of clients, rooms, and specialists.
- Ensure GDPR compliance by enabling secure note-taking on appointments by any of the stakeholders: the receptionist, specialist, or client.

Refer to individual components:

[About booking](booking/README.md)  
[About the frontend](booking/frontend/README.md)  
[About the backend](booking/backend/README.md)  

## Business domain

The application is generic, suitable for any domain that requires appointments, from medical to consultative. Its key feature is ensuring seamless bookings while maintaining GDPR compliance for any notes taken during appointments.

## Technologies Used

### Frontend

- **React App**: Hosted on an AWS S3 bucket. Deployment involves building the project within a container and subsequently uploading all assets to the bucket.

### Backend

- **ASP.NET API**: Runs using AWS Fargate within the Elastic Container Service (ECS).
- **PostgreSQL Database**: Deployed as an RDS instance on AWS.
- **Database Migrations**: Managed using AWS Lambda.

### Infrastructure

- Everything is securely hosted within an AWS Virtual Private Cloud (VPC).
- The VPC spans across two subnets.
- Deployment and infrastructure management are executed using Pulumi.
- Continuous Integration and Continuous Deployment (CI/CD) are facilitated through GitHub Actions.

### Other Technologies

- **Pulumi**: Infrastructure as Code (IaC) tool for cloud services, with AWS as the provider for this project.
- **Docker**: Containers ensure a consistent environment for both development and deployment.

## Setup

Setting up a local dev environment is distinct from deployments. Detailed instructions for both scenarios will be provided soon.
