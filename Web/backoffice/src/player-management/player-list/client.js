import useSWR from "swr"
import client, { urlWith, get, put, post } from "../../client"

export const useList = (searchParams = {}) => {
  let url = urlWith(searchParams, `/api/player`)
  const { data, error, isValidating, mutate } = useSWR(url, get)

  return {
    data,
    isLoading: !error && !data,
    isValidating,
    error,
    isError: error,
    mutate
  }
}

export const usePlayer = ({ playerCode }) => {
  const { data, error, isValidating, mutate } = useSWR(
    playerCode ? `/api/player/${playerCode}` : null,
    (url) => get(url).then((data) => data.player)
  )

  return {
    player: data,
    isLoading: !error && !data,
    isValidating,
    error,
    isError: error,
    mutate
  }
}

export const useEditPlayerInfo = ({ playerCode, data }) => {
  let fetchData = playerCode && data
  const { data: result, error, isValidating, mutate } = useSWR(
    fetchData ? `/api/player/${playerCode}/info` : null,
    (url) => put(url, { data })
  )

  return {
    result,
    isLoading: !error && !result && fetchData,
    isValidating,
    error,
    isError: error,
    mutate
  }
}

export const useEditPlayerContact = ({ playerCode, data }) => {
  let fetchData = playerCode && data
  const { data: result, error, isValidating, mutate } = useSWR(
    fetchData ? `/api/player/${playerCode}/contact` : null,
    (url) => put(url, { data })
  )

  return {
    result,
    isLoading: !error && !result && fetchData,
    isValidating,
    error,
    isError: error,
    mutate
  }
}

export const useAddPlayerBank = ({ playerCode, data }) => {
  let fetchData = playerCode && data
  const { data: result, error, isValidating, mutate } = useSWR(
    fetchData ? `/api/player/${playerCode}/bank` : null,
    (url) => post(url, { data })
  )

  return {
    result,
    isLoading: !error && !result && fetchData,
    isValidating,
    error,
    isError: error,
    mutate
  }
}

export const useEditPlayerBank = ({ playerCode, id, data }) => {
  let fetchData = playerCode && id && data
  const { data: result, error, isValidating, mutate } = useSWR(
    fetchData ? `/api/player/${playerCode}/bank/${id}` : null,
    (url) => put(url, { data })
  )

  return {
    result,
    isLoading: !error && !result && fetchData,
    isValidating,
    error,
    isError: error,
    mutate
  }
}

export const useDeletePlayerBank = ({ playerCode, id }) => {
  let fetchData = playerCode && id
  const { data: result, error, isValidating, mutate } = useSWR(
    fetchData ? `/api/player/${playerCode}/bank/${id}` : null,
    (url) => client.delete(url)
  )

  return {
    result,
    isLoading: !error && !result && fetchData,
    isValidating,
    error,
    isError: error,
    mutate
  }
}

export const useEditPlayerPassword = ({ playerCode, data }) => {
  const fetchData = playerCode && data?.password
  const { data: result, error, isValidating, mutate } = useSWR(
    fetchData ? [`/api/player/${playerCode}/password`, data] : null,
    (url, data) => put(url, { data })
  )

  return {
    result,
    isLoading: !error && !result && fetchData,
    isValidating,
    error,
    isError: error,
    mutate
  }
}

export const useOperations = ({ playerCode, searchParams = {} }) => {
  let url = playerCode
    ? urlWith(searchParams, `/api/player/${playerCode}/operations`)
    : null

  const { data, error, isValidating, mutate } = useSWR(url, get)

  return {
    data: data ?? {},
    isLoading: !error && !data,
    isValidating,
    error,
    isError: error,
    mutate
  }
}
