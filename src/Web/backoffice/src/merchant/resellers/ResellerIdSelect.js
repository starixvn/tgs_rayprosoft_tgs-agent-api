import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useFormContext, useFieldArray, Controller } from "react-hook-form"
import { Table, Set, Heading, Text, useModalState, Icon, Box } from "bumbag"

export const ResellerIdSelect = ({
  name = "resellerId",
  selectRef,
  defaultValue,
  value,
  required,
  state,
  ...props
}) => {
  let formMethods = useFormContext()

  let { data, isLoading } = useResellerList({ showAll: true })
  let options =
    data?.items.map((reseller) => ({
      label: reseller.code,
      value: reseller.id
    })) ?? []

  state = state ?? (formMethods?.errors?.[name] && "danger")

  let placeholder = useResellerPlaceholder()
  return (
    <Select
      name={name}
      required={required}
      selectRef={selectRef ?? formMethods?.register({ required })}
      state={state}
      isLoading={isLoading}
      options={options}
      placeholder={placeholder}
      {...props}
    />
  )
}

export default ResellerIdSelect
