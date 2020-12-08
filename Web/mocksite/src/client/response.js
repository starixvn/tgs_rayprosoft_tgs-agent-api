export const toDataOrError = (response) => {
  if (!response.ok) {
    return response
      .json()
      .catch((error) => {
        return Promise.reject({
          title: response.statusText
        })
      })
      .then((data) => {
        return Promise.reject(data)
      })
  }

  return response.json()
}
