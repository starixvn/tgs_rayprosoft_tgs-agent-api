import useSWR from "swr"
import client, { urlWith, get, put, post, useGet } from "../../client"

//TODO
const API = ""

export const useProviderList = (searchParams) => {
  let url = searchParams ? urlWith(searchParams, API) : ""
  return useGet(url)
}
