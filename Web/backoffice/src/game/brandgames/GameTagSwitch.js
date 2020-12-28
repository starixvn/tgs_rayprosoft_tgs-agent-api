import React, { useState, useEffect, useMemo } from "react"
import {
  css,
  Stack,
  Heading,
  Box,
  Button,
  Select,
  SelectMenu,
  DropdownMenu,
  Switch,
  Spinner
} from "bumbag"
import { useTrans, Trans } from "../../i18n"
import { useUpdateBrandGameTag } from "./client"

export const GameTagSwitch = ({ id, tag, checked, onSubmitted, ...props }) => {
  let [formData, setSubmit] = useState()
  let { result, isLoading, error, deleteCache } = useUpdateBrandGameTag(
    formData
  )

  useEffect(() => {
    if (result) {
      onSubmitted?.()
      deleteCache()
    }
  }, [result])

  if (isLoading) {
    return <Spinner />
  }

  return (
    <Switch
      checked={checked}
      onChange={(event) => {
        setSubmit({
          id,
          tag,
          enabled: event.target.checked
        })
      }}
    />
  )
}
