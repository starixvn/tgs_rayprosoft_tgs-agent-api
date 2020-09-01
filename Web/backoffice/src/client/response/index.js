//
export const toDataOrError = (response) => {
  if (!response?.ok) {
    throw new Error(response.statusText)
  }

  let promise = response?.json() ?? Promise.resolve({})
  return promise.then((data) => {
    if (!data?.valid && data?.message) {
      throw new Error(data?.message)
    }

    return data
  })
}

const defaultMap = (data) => data
