resource "aws_lambda_function" "contact_form_api_function" {
  filename      = "${var.aws_prefix}_contact_form_api_function_${var.environment}.zip"
  function_name = "${var.aws_prefix}_contact_form_api_function_${var.environment}"
  role          = aws_iam_role.iam_for_lambda_role.arn
  runtime       = "dotnet8"
  handler       = var.lambda_function_handler_name

  environment {
    variables = {
      ASPNETCORE_ENVIRONMENT = var.environment
      aws__Region            = var.region
      smtpInfo__Host         = var.smtpInfo_Host
      smtpInfo__Port         = var.smtpInfo_Port
      smtpInfo__DisplayName  = var.smtpInfo_DisplayName
      smtpInfo__Username     = var.smtpInfo_Username
      smtpInfo__Password     = var.smtpInfo_Password
      smtpInfo__UseSSL       = var.smtpInfo_UseSSL
      profile__Name          = var.profile_Name
      profile__Email         = var.profile_Email
      profile__Contact       = var.profile_Contact
      profile__LinkedIn      = var.profile_LinkedIn
      profile__GitHub        = var.profile_GitHub
      profile__WhatsApp      = var.profile_WhatsApp
      profile__Address       = var.profile_Address
      profile__Website       = var.profile_Website
      cors__Origins          = var.cors_Origins
      cors__Methods          = var.cors_Methods
    }
  }

  tags = {
    "deployment:source"        = var.tags_deployment_source
    "deployment:type"          = "terraform"
    "reference:owner"          = var.tags_reference_owner
    "reference:domain"         = var.tags_reference_domain
    "reference:subdomain"      = var.tags_reference_subdomain
    "reference:application"    = var.tags_reference_application
    "reference:authentication" = var.tags_reference_authentication
    "reference:contact"        = var.tags_reference_contact
  }
}
