output "function_url" {
  value = aws_lambda_function.lambda_function.invoke_arn
}
