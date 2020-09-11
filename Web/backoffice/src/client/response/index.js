export const toDataOrError = (response) => {
  if (!response?.ok) {
    if (response.statusText === "Unauthorized") {
      location.href = "/login"
    } else {
      throw new Error(response.statusText)
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
