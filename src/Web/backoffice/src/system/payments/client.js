import useSWR, { cache } from "swr"
import { urlWith, useGet, usePostData, usePutData } from "../../client"

const PaymentAPI = "/api/payment"

export const usePaymentList = (searchParams) => {
  let url = searchParams ? urlWith(searchParams, PaymentAPI) : ""
  return useGet(url)
}

export const usePayment = (id) => {
  let url = id ? `${PaymentAPI}/${id}` : ""
  return useGet(url)
}

export const useAddPayment = (data) => {
  return usePostData(PaymentAPI, data)
}

export const useUpdatePayment = (id, data) => {
  let url = id ? `${PaymentAPI}/${id}` : ""
  return usePutData(url, data)
}
