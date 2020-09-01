import { toDataOrError } from "./response"

export const post = (
  url,
  { body, data = {}, headers = {}, ...options } = {}
) => {
  return fetch(url, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      ...headers
    },
    body: body ?? JSON.stringify(data),
    ...options
  }).then(toDataOrError)
}

export default {
  post
}
