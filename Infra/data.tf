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

data "aws_iam_policy" "lambda_execution_access" {
  arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
}

data "archive_file" "lambda" {
  type        = "zip"
  source_dir  = "${path.module}/../src/bin/Release/net8.0"
  output_path = "${path.module}/publish.zip"
}

resource "null_resource" "sync_s3_bucket" {
  depends_on = [data.archive_file.lambda]

  triggers = {
    app_version = var.app_version
  }

  provisioner "local-exec" {
    command = "aws s3 cp ${data.archive_file.lambda.output_path} s3://${var.bucket_name}/lambda/${var.app_name}/${var.app_version}/${var.environment}/Publish.zip"
  }
}
