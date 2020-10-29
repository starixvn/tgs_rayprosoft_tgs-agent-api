import useSWR, { cache } from "swr"
import client, { urlWith, get, put, post } from "../../client"

export const useList = (searchParams) => {
  let url = urlWith(searchParams, `/api/role`)
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

export const useRole = ({ id, shouldFetch = true } = {}) => {
  let fetchData = shouldFetch && !!id
  let key = `/api/role/${id}`
  const { data: role, error, isValidating } = useSWR(
    fetchData ? key : null,
    (url) => get(url).then(({ role, ...response }) => role)
  )

  return {
    role,
    isLoading: !error && !role && fetchData,
    isValidating,
    error,
    isError: error,
    mutate: () => {
      cache.delete(key)
    }
  }
}

export const useAddRole = ({ data }) => {
  let fetchData = !!data
  let { data: result, error, isValidating, mutate } = useSWR(
    fetchData ? [`/api/role`, data] : null,
    (url, data) => post(url, { data })
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

export const useEditRole = ({ id, data }) => {
  let fetchData = id && !!data
  let keys = fetchData ? [`/api/role/${id}`, data] : null
  let { data: result, error, isValidating, mutate } = useSWR(
    keys,
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

export const useCopyRole = ({ id, data }) => {
  let fetchData = id && data
  let keys = fetchData && [`/api/role/${id}/copy`, data]
  const { data: result, error, isValidating, mutate } = useSWR(
    keys,
    (url, data) => post(url, { data })
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

export const useDeleteRole = ({ id }) => {
  let fetchData = id
  let url = fetchData && `/api/role/${id}`
  let { data: result, error, isValidating, mutate } = useSWR(url, client.delete)

  return {
    result,
    isLoading: !error && !result && fetchData,
    isValidating,
    error,
    isError: error,
    mutate
  }
}

export const useMenus = ({ shouldFetch = true } = {}) => {
  let key = "/api/role/menus"
  const { data: menus, error, isValidating, mutate } = useSWR(
    shouldFetch ? key : null,
    (url) =>
      post(url).then(({ menus, ...response }) => resetDisplay(false, menus))
  )

  return {
    menus,
    isLoading: !error && !menus,
    isValidating,
    error,
    isError: error,
    mutate: () => cache.delete(key)
  }
}

const resetDisplay = (isDisplay, menus = []) => {
  return menus.map((menu) => {
    menu.isDisplay = isDisplay
    if (menu.subMenus) {
      menu.subMenus = resetDisplay(isDisplay, menu.subMenus)
    }
    return menu
  })
}
