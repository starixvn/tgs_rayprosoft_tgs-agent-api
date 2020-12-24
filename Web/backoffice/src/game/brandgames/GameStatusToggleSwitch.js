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
import { GameStatus } from "../games/GameStatus"
import { useChangeBrandGameStatus } from "./client"
import { mutate } from "swr"

export const GameStatusToggleSwitch = ({
  id,
  value,
  onSubmitted,
  ...props
}) => {
  let isActive = value == GameStatus.Active

  let [formData, setSubmit] = useState()
  let { result, isLoading, error, deleteCache } = useChangeBrandGameStatus(
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
      checked={isActive}
      onChange={(event) => {
        setSubmit({
          id,
          status: event.target.checked ? GameStatus.Active : GameStatus.Inactive
        })
      }}
    />
  )
}
