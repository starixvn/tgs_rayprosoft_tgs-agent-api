import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useModalState, Stack, Heading, Box, Text, FieldStack } from "bumbag"
import FormDialog from "../../form/FormDialog"
import {
  useFormContext,
  useFieldArray,
  Controller,
  useForm
} from "react-hook-form"
import Backdrop from "../../dialog/Backdrop"
import Disclosure from "../../dialog/Disclosure"
import { Trans } from "../../i18n"
import Checkbox from "../../checkbox"

export const BrandGamePoolDisclosure = ({
  name = "brandGames",
  fieldArrayName = "games",
  disabled = false,
  ...props
}) => {
  let dialog = useModalState()
  let { control, setValue, watch, register } = useFormContext()
  let games = watch(fieldArrayName)

  return (
    <Controller
      control={control}
      name={name}
      defaultValue={games}
      render={(
        { onChange, onBlur, value, name, ref },
        { invalid, isTouched, isDirty }
      ) => {
        return (
          <>
            <Disclosure {...dialog}>
              <Trans zh="游戏列表" en="Games" />
            </Disclosure>
            <BrandGamePoolDialog
              games={value}
              dialog={dialog}
              onChange={(games) => {
                setValue(name, games)
              }}
            />
          </>
        )
      }}
    />
  )
}

export const BrandGamePoolDialog = ({ games = [], onChange, dialog }) => {
  let formMethods = useForm({
    defaultValues: {
      games
    }
  })

  let { register, unregister, setValue, control } = formMethods
  let { fields } = useFieldArray({
    control,
    name: "games",
    keyName: "_id"
  })

  const onSubmit = (data) => {
    let state = games.map(({ gameId, ...game }) => {
      return {
        ...game,
        gameId,
        checked: data[gameId]
      }
    })
    onChange(state)
    dialog.hide()
  }

  return (
    <Backdrop {...dialog}>
      <FormDialog
        formMethods={formMethods}
        stopPropagation={true}
        onSubmit={onSubmit}
        dialog={dialog}
      >
        {fields.map((field) => (
          <Checkbox
            key={field._id}
            name={`${field.gameId}`}
            checkboxRef={register?.()}
            defaultChecked={field.checked}
            label={field.gameId}
          />
        ))}
      </FormDialog>
    </Backdrop>
  )
}
