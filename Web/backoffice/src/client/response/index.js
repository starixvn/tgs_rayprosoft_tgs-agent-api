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
          let { title, message, errors = [] } = data
          return Promise.reject({
            title: title ?? message ?? response.statusText,
            errors: Object.values(errors)
          })
        })
    }
  }

  return response.json().then((data) => {
    if (!data?.valid && data?.message) {
      return Promise.reject({
        title: data.message
      })
    }

    return data
  })
}
