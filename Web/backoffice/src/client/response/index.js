export const toDataOrError = (response) => {
  if (!response?.ok) {
    if (response.statusText === "Unauthorized") {
      location.href = "/login"
    } else {
      return response
        .json()
        .catch((error) => {
          return Promise.reject({
            title: response.statusText
          })
        })
        .then((data) => {
          let { title, errors = [] } = data
          return Promise.reject({
            title: title ?? response.statusText,
            errors: Object.values(errors)
          })
        })
    }
  }

  let promise = response?.json() ?? Promise.resolve({})
  return promise.then((data) => {
    if (!data?.valid && data?.message) {
      throw new Error(data?.message)
    }

    return data
  })
}
