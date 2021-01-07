import React, { useState, useEffect, useMemo, useRef } from "react"
import { usePaymentMethodList } from "./client"
import AlertDialog from "../../../dialog/Alert"
import { Select as _Select } from "../../../select/Select"

export const Select = ({ required, ...props }) => {
  //TODO: need filters? payment vendor etc.
  let { data, isLoading, error } = usePaymentMethodList({})

  console.warn("TODO: map item for i18n?")
  let options = data?.items.map((item) => item) ?? []

  if (!required) {
    options = [EmptyOption, ...options]
  }

  return (
    <>
      {error && <AlertDialog {...error} />}
      <_Select isLoading={isLoading} options={options} {...props} />
    </>
  )
}

export const Field = ({ name, required, ...fieldProps }) => {
  return (
    <SelectField name={name} required={required} {...fieldProps}>
      {(selectProps) => <Select required={required} {...selectProps} />}
    </SelectField>
  )
}

export default Select
