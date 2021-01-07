import React, { useState, useEffect, useMemo, useRef } from "react"
import { usePaymentList } from "./client"
import AlertDialog from "../../../dialog/Alert"
import { Select as _Select } from "../../select/Select"

export const Select = ({ ...props }) => {
  let { data, isLoading, error } = usePaymentList()

  console.warn("TODO: map item for i18n?")
  let options = data?.items.map((item) => item) ?? []

  return (
    <>
      {error && <AlertDialog {...error} />}
      <_Select isLoading={isLoading} options={options} {...props} />
    </>
  )
}

export default Select
