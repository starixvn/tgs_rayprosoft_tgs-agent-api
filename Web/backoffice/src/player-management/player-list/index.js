import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/core"
import { Formik, Field } from "formik"
import {
  Heading,
  Stack,
  Box,
  Button,
  InputField,
  Set,
  useModalState
} from "bumbag"
import PageContent from "../../page/Content"
import sequenceColumn from "../../table/column/sequence"
import dateTimeColumn from "../../table/column/dateTime"
import { usePlayerList } from "./client"
import { useTable, Table, pageCount } from "../../table"
import { Label as StatueLabel } from "../status"
import {
  useIsDisplayMenuByRoute,
  useHasSubMenuByRoute
} from "../../user/context"
import { Code } from "./menu"
import { Field as StatusField } from "../status"
import { Field as VipLevelField } from "../../vip-levels/Select"
import { ViewButton, EditButton } from "./button"
import { Trans, useTrans } from "../../i18n"
import SearchButton from "../../button/SearchButton"
import equals from "fast-deep-equal"

export { Code }

export const Content = () => {
  let isDisplay = useIsDisplayMenuByRoute()
  if (!isDisplay) {
    return null
  }

  return <AsyncContent />
}

export const CodeContentPair = [Code, Content]

const AsyncContent = () => {
  let { state, setState, setPageIndex, setSortOrder } = useTable({
    status: null,
    vipId: null,
    username: ""
  })
  let {
    pageIndex,
    pageSize,
    sortby,
    orderby,
    status,
    vipId,
    username = ""
  } = state
  let { data = {}, isLoading, isValidating, mutate } = usePlayerList({
    page: pageIndex + 1,
    rows: pageSize,
    sortby,
    orderby,
    status,
    vipId,
    username
  })

  let columns = useTableColumns({ fetchData: mutate })

  return (
    <PageContent
      title={<Trans zh="玩家列表" en="Players" />}
      actionBar={
        <SearchForm
          search={(data) => {
            let newState = {
              pageIndex: 0,
              status: data.status,
              vipId: data.vipId,
              username: data.username
            }
            if (equals(newState, { pageIndex, status, vipId, username })) {
              mutate()
            }
            setState({
              pageIndex: 0,
              ...data
            })
          }}
          isPending={isLoading}
        />
      }
    >
      <Table
        controlledState={state}
        columns={columns}
        data={data?.items}
        pageCount={pageCount(pageSize, data?.totalCount)}
        setPageIndex={setPageIndex}
        setSortOrder={setSortOrder}
        isLoading={isLoading}
      />
    </PageContent>
  )
}

const SearchForm = ({ search, isPending, ...props }) => {
  let usernameInputPlaceholder = useSearchInputPlaceHolder()
  return (
    <Formik
      initialValues={{ status: "", vipId: "", username: "" }}
      onSubmit={(data) => {
        search(data)
      }}
    >
      {(formProps) => (
        <form onReset={formProps.handleReset} onSubmit={formProps.handleSubmit}>
          <Set>
            <StatusField name="status" />
            <VipLevelField name="vipId" />
            <Field
              component={InputField.Formik}
              name="username"
              addonAfter={
                <Button onClick={() => formProps.setFieldValue("username", "")}>
                  X
                </Button>
              }
              placeholder={usernameInputPlaceholder}
              autocomplete="off"
            />
            <SearchButton isLoading={isPending} />
          </Set>
        </form>
      )}
    </Formik>
  )
}

const useSearchInputPlaceHolder = () =>
  useTrans({ zh: "用户名输入框", en: "Input user name" })

const useTableColumns = ({ fetchData } = {}) => {
  let columns = [
    sequenceColumn(),
    {
      Header: <Trans zh="用户名" en="User name" />,
      accessor: "username",
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      }
    },
    {
      Header: <Trans zh="VIP 等级" en="VIP Level" />,
      disableSortBy: true,
      accessor: "vipLevel",
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      }
    },
    {
      disableSortBy: true,
      Header: <Trans zh="状态" en="Status" />,
      accessor: "status",
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value }) => <StatueLabel value={value} />
    }
  ]

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
          <Box alignX="center">
            <Set>
              <ViewButton player={value} />
              <EditButton player={value} onSubmitted={fetchData} />
            </Set>
          </Box>
        )
      }
    })
  }

  return useMemo(() => columns)
}
