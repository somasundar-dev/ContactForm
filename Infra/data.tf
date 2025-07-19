data "aws_iam_policy_document" "assume_role" {
  statement {
    effect = "Allow"

    principals {
      type        = "Service"
      identifiers = ["lambda.amazonaws.com"]
    }

    actions = ["sts:AssumeRole"]
  }
}

data "archive_file" "lambda" {
  type        = "zip"
  source_dir  = "${path.module}/../src/ContactForm.API/bin/Release/net8.0"
  output_path = "${var.aws_prefix}_contact_form_api_function_${var.environment}.zip"
}

data "aws_iam_policy" "dynamodb_execution_access" {
  arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaDynamoDBExecutionRole"
}

data "aws_iam_policy" "lambda_execution_access" {
  arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
}
