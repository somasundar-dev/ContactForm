resource "aws_lambda_function" "contact_form_api_function" {
  filename      = "${var.aws_prefix}_contact_form_api_function_${var.environment}.zip"
  function_name = "${var.aws_prefix}_contact_form_api_function_${var.environment}"
  role          = aws_iam_role.iam_for_lambda_role.arn
  runtime       = var.dotnet_runtime
  handler       = var.lambda_function_handler_name

  environment {
    variables = {
      ASPNETCORE_ENVIRONMENT     = var.environment
      aws__Region                = var.region
      EmailSettings__Host        = var.smtpInfo_Host
      EmailSettings__Port        = var.smtpInfo_Port
      EmailSettings__DisplayName = var.smtpInfo_DisplayName
      EmailSettings__Username    = var.smtpInfo_Username
      EmailSettings__Password    = var.smtpInfo_Password
      EmailSettings__UseSSL      = var.smtpInfo_UseSSL
      UserInfo__Name             = var.profile_Name
      UserInfo__Email            = var.profile_Email
      UserInfo__Contact          = var.profile_Contact
      UserInfo__LinkedIn         = var.profile_LinkedIn
      UserInfo__GitHub           = var.profile_GitHub
      UserInfo__Address          = var.profile_Address
      UserInfo__Website          = var.profile_Website
      AllowedOrigins             = var.cors_Origins
    }
  }

  tags = {
    "deployment:source" = var.tags_deployment_source
    "deployment:type"   = "terraform"
  }
}
