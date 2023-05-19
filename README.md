# Care Calendar
Webapp to manage appointments

## Concerns
These are the focus points over what we're trying to achieve with this project.

### Security
Follow the GDPR, HIPAA and POPIA guidelines to ensure that the personal data of the users is protected.

## Tech
1. solidjs frontend (aws s3 bucket + cloudfront)
2. c# api (aws ecs)
3. postresql db (aws rds)
4. ci/cd pipeline (gh actions)
5. pulumi iac


## Deployment
There are 3 github actions workflows that are triggered on push to the main branch:
