import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/core"
import { Formik, Field } from "formik"
import { Heading, Stack, Button, InputField, Set, Text } from "bumbag"
import PageContent from "../../page/Content"
import { AddButton, EditButton, EditPasswordButton } from "./edit"
import sequenceColumn from "../../table/column/sequence"
import dateTimeColumn from "../../table/column/dateTime"
import { Label as StatusLabel } from "./status"
import {
  useIsDisplayMenuByRoute,
  useHasSubMenuByRoute
} from "../../user/context"
import { Code } from "./menu"
import { useAdminList } from "./client"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import { Trans, useTrans } from "../../i18n"
import SearchButton from "../../button/Search"
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
    username: ""
  })

  let { pageIndex, pageSize, username = "" } = state

  let { data, isLoading, mutate } = useAdminList(toApiParams(state))
  const fetchTable = mutate
  let columns = useTableColumns({ fetchTable })

  return (
    <PageContent
      title={<Trans zh="管理员列表" en="Admins" />}
      actionBar={
        <>
          <SearchForm
            search={(data) => {
              let newState = {
                pageIndex: 0,
                username: data?.username
              }
              if (equals(newState, { pageIndex, username })) {
                mutate()
              }
              setState(newState)
            }}
            isPending={isLoading}
          />
          <AddButton onSubmitted={fetchTable} />
        </>
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
  return (
    <Formik
      initialValues={{ username: "" }}
      onSubmit={(data) => {
        search(data)
      }}
    >
      {(formProps) => (
        <form onReset={formProps.handleReset} onSubmit={formProps.handleSubmit}>
          <Set>
            <Field
              component={InputField.Formik}
              name="username"
              addonAfter={
                <Button onClick={() => formProps.setFieldValue("username", "")}>
                  X
                </Button>
              }
              placeholder={searchInputPlaceHolder()}
              autocomplete="off"
            />
            <SearchButton isLoading={isPending} />
          </Set>
        </form>
      )}
    </Formik>
  )
}

const searchInputPlaceHolder = () =>
  useTrans({
    zh: "管理员用户名输入框",
    en: "Input admin user name"
  })

const useTableColumns = ({ fetchTable }) => {
  let columns = [
    sequenceColumn(),
    {
      Header: <Trans zh="用户名" en="Username" />,
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
      Header: <Trans zh="姓名" en="Name" />,
      accessor: "name",
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      }
    },
    {
      Header: <Trans zh="部门" en="Department" />,
      accessor: "departmentName",
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center",
        whiteSpace: "nowrap"
      }
    },
    {
      Header: <Trans zh="权限组" en="Role" />,
      accessor: "role",
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center",
        whiteSpace: "nowrap"
      }
    },
    {
      Header: <Trans zh="最近登入IP" en="Last Login IP" />,
      accessor: "lastLoginIp",
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      }
    },
    dateTimeColumn({
      Header: <Trans zh="最近登入时间" en="Last Login Time" />,
      accessor: "lastLoginTime",
      disableSortBy: true
    }),
    dateTimeColumn({
      Header: <Trans zh="创建时间" en="Created On" />,
      accessor: "createdOn",
      disableSortBy: true
    }),
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
      Cell: ({ value }) => (
        <Text.Block
          css={css`
            white-space: nowrap;
          `}
        >
          <StatusLabel value={value} />
        </Text.Block>
      )
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
          <Set>
            <EditButton user={value} onSubmitted={fetchTable} />
            <EditPasswordButton user={value} onSubmitted={fetchTable} />
          </Set>
        )
      }
    })
  }

  return useMemo(() => columns)
}
