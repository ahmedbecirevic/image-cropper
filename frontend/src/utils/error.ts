interface ApiError {
  response?: {
    data?: {
      message?: string
    }
  }
  message?: string
}

export function getErrorMessage(err: unknown, defaultMessage: string): string {
  if (err && typeof err === 'object' && 'response' in err) {
    const apiError = err as ApiError
    return apiError.response?.data?.message || defaultMessage
  }

  if (err instanceof Error) {
    return err.message
  }

  return defaultMessage
}
