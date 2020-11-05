import useSWR from "swr"
import client, { urlWith, get, put, post, useGet } from "../../client"

export const useProviderList = (searchParams) => {
  let url = searchParams ? urlWith(searchParams, PaymentAPI) : ""
  return useGet(url)
}
