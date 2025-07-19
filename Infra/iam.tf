resource "aws_iam_role" "iam_for_lambda_role" {
  name               = "${var.aws_prefix}_iam_for_lambda_role_${var.environment}"
  assume_role_policy = data.aws_iam_policy_document.assume_role.json
}

resource "aws_iam_role_policy_attachment" "lambda_execution_access" {
  role       = aws_iam_role.iam_for_lambda_role.name
  policy_arn = data.aws_iam_policy.lambda_execution_access.arn
}

resource "aws_iam_role_policy_attachment" "dynamodb_execution_access" {
  role       = aws_iam_role.iam_for_lambda_role.name
  policy_arn = data.aws_iam_policy.dynamodb_execution_access.arn
}
