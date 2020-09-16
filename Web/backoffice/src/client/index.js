import { toDataOrError } from "./response"

export const get = (url = "", { searchParams = {}, ...options } = {}) => {
  let urlSearchParams = Object.entries(searchParams).reduce(
    (searchParams, [key, value]) => {
      if (typeof value == "string" && !value) {
        return searchParams
      }

      return searchParams
        ? `${searchParams}&${key}=${value}`
        : `${key}=${value}`
    },
    ""
  )
  url = `${url}?${urlSearchParams}`
  return fetch(url, options).then(toDataOrError)
}

const fetchApi = (
  method = "",
  url,
  { body, data = {}, headers = {}, ...options } = {}
) =>
  fetch(url, {
    method,
    headers: {
      "Content-Type": "application/json",
      ...headers
    },
    body: body ?? JSON.stringify(data),
    ...options
  }).then(toDataOrError)

export default {
  get,
  put: (url, options) => fetchApi("PUT", url, options),
  post: (url, options) => fetchApi("POST", url, options),
  delete: (url, options) => fetchApi("DELETE", url, options)
}
