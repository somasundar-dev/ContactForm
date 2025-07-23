resource "aws_lambda_function" "lambda_function" {
  depends_on = [
    aws_iam_role.iam_for_lambda_role,
    aws_iam_role_policy_attachment.lambda_execution_access,
    null_resource.sync_s3_bucket
  ]
  function_name = "${var.aws_prefix}-${var.app_name}-function-${var.environment}"
  role          = aws_iam_role.iam_for_lambda_role.arn
  runtime       = var.dotnet_runtime
  handler       = var.lambda_function_handler_name
  timeout       = 30

  # Use existing S3 bucket + key
  s3_bucket = var.bucket_name
  s3_key    = "lambda/${var.app_name}/${var.app_version}/${var.environment}/Publish.zip"

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
    "deployment:source"      = var.tags_deployment_source
    "deployment:type"        = "terraform"
    "deployment:app"         = var.app_name
    "deployment:version"     = var.app_version
    "deployment:environment" = var.environment
    "deployment:region"      = var.region
  }
}
