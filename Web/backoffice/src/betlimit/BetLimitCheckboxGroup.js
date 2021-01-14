import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Table, Set, Heading, Text } from "bumbag"
import { useBetLimitList } from "./client"
import Checkbox from "../checkbox"
import { useFormContext, useFieldArray } from "react-hook-form"
import { isAllChecked } from "../checkbox/isAllChecked"
import { Trans } from "../i18n"

const DefaultName = "betLimitIds"

export const BetLimitCheckboxGroup = ({
  name = DefaultName,
  arrayFieldName = "betLimits",
  width = "100%",
  disabled,
  ...props
}) => {
  let { control, setValue, watch } = useFormContext()
  const { fields: betLimitFields } = useFieldArray({
    control,
    name: arrayFieldName,
    keyName: "_id"
  })

  let betLimitIds = watch(name) ?? {}
  let allChecked = isAllChecked(betLimitIds)
  return (
    <Set orientation="vertical" width={width} {...props}>
      <Table width={width}>
        <Table.Head>
          <Table.Row>
            <Table.HeadCell>
              <Checkbox
                checked={allChecked}
                onChange={(event) => {
                  let checked = event.target.checked
                  betLimitFields.forEach((field, index) => {
                    setValue(fieldName(name, field), checked)
                  })
                }}
                disabled={disabled}
              />
            </Table.HeadCell>
            <Table.HeadCell>
              <Trans zh="币种" en="Currency" />
            </Table.HeadCell>
            <Table.HeadCell>
              <Trans zh="最低投注" en="Min Bet" />
            </Table.HeadCell>
            <Table.HeadCell>
              <Trans zh="最高投注" en="Max Bet" />
            </Table.HeadCell>
            <Table.HeadCell>
              <Trans zh="单场最高投注" en="Max Bet Per Match" />
            </Table.HeadCell>
          </Table.Row>
        </Table.Head>
        <Table.Body>
          {betLimitFields.map((betLimitField) => (
            <BetLimitRow
              key={betLimitField._id}
              name={name}
              disabled={disabled}
              field={betLimitField}
            />
          ))}
        </Table.Body>
      </Table>
    </Set>
  )
}

const fieldName = (name = DefaultName, checkboxField) =>
  `${name}.${checkboxField.id}`

const BetLimitRow = ({ name, disabled, field, ...props }) => {
  let {
    id,
    currencyCode,
    currencyName,
    minBet,
    maxBet,
    maxBetPerMatch,
    checked
  } = field
  let formContext = useFormContext()
  return (
    <Table.Row>
      <Table.Cell>
        <Checkbox
          key={field._id}
          name={fieldName(name, field)}
          checkboxRef={formContext?.register?.()}
          defaultChecked={checked}
          disabled={disabled}
        />
      </Table.Cell>
      <Table.Cell>{currencyCode}</Table.Cell>
      <Table.Cell>{minBet}</Table.Cell>
      <Table.Cell>{maxBet}</Table.Cell>
      <Table.Cell>{maxBetPerMatch}</Table.Cell>
    </Table.Row>
  )
}
