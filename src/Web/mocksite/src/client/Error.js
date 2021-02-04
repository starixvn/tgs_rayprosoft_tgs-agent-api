export const InvalidCredentials = 101

export const InvalidOrExpiredPlayerToken = 102

export const Code = {
  InvalidCredentials,
  InvalidOrExpiredPlayerToken
}

export const handleError = ({
  brandTokenHook = {},
  playerTokenHook = {}
} = {}) => (error) => {
  if (error?.err === InvalidCredentials) {
    brandTokenHook?.mutate?.()
  }

  if (error?.err === InvalidOrExpiredPlayerToken) {
    playerTokenHook?.mutate?.()
  }

  return Promise.reject(error)
}
