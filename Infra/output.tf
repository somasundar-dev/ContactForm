output "function_url" {
  value = aws_lambda_function.contact_form_api_function.invoke_arn
}
