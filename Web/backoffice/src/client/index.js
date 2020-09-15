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

export const put = (
  url,
  { body, data = {}, headers = {}, ...options } = {}
) => {
  return fetch(url, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      ...headers
    },
    body: body ?? JSON.stringify(data),
    ...options
  }).then(toDataOrError)
}

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
  get,
  put,
  post
}
