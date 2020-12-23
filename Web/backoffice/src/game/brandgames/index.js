import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { Formik, Form, Field, useFormikContext } from "formik"
import { Heading, Stack, Button, InputField, Set, Text, Box } from "bumbag"
import PageContent from "../../page/Content"
import sequenceColumn from "../../table/column/sequence"
import dateTimeColumn from "../../table/column/dateTime"
import {
  useIsDisplayMenuByRoute,
  useHasSubMenuByRoute
} from "../../user/context"
import { Code } from "./menu"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import { Trans, useTrans } from "../../i18n"
import { SearchButton } from "../../button/SearchButton"
import equals from "fast-deep-equal"
import { useBrandGameList } from "./client"
import { useFormattedTimeString } from "../../date/format"
import { useFormattedDay } from "../../date/week"
import formatISO from "date-fns/fp/formatISO"
import { StatusMenu } from "./status"
import { EditButton } from "./edit"

export { Code } from "./menu"

export const Content = () => {
  let isDisplay = useIsDisplayMenuByRoute()
  if (!isDisplay) {
    return null
  }

  return <Body />
}

export const CodeContentPair = [Code, Content]

export const Body = () => {
  return ""
}

const useColumns = ({ items = [], mutate }) => {
  let columns = [
    {
      accessor: "id",
      Header: <Trans zh="ID" en="ID" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      }
    },
    {
      accessor: "name",
      Header: <Trans zh="厂商名称" en="Name" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      }
    },
    {
      id: "maintenanceTime",
      accessor: (record) => record,
      Header: <Trans zh="定期维护时间" en="Periodic Maintenance Time" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value }) => {
        let { day, start, end } = value
        return (
          <Text.Block whiteSpace="nowrap">
            {`${useFormattedDay(day)} ${start} - ${end}`}
          </Text.Block>
        )
      }
    },
    {
      accessor: "status",
      Header: <Trans zh="状态" en="Status" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value }) => {
        return (
          <Box alignX="center">
            <StatusMenu value={value} />
          </Box>
        )
      }
    }
  ]

  //TODO: auth
  let hasAnyMenu = useHasSubMenuByRoute()
  if (hasAnyMenu) {
    columns.push({
      id: "actions",
      disableSortBy: true,
      Header: <Trans zh="操作" en="Actions" />,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      accessor: (row) => {
        return row
      },
      Cell: ({ value }) => {
        return (
          <Set orientation="vertical" alignX="center">
            <EditButton record={value} onSubmitted={mutate} />
            <EditPresetButton />
          </Set>
        )
      }
    })
  }

  return useMemo(() => columns, [JSON.stringify(items)])
}
