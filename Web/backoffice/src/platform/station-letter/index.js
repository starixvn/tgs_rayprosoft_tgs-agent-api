import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/core"
import { Box, Set, Button, Text, Switch, Input, Stack } from "bumbag"
import PageContent from "../../page/Content"
import {
  useIsDisplayMenuByRoute,
  useHasSubMenuByRoute
} from "../../user/context"
import { useStationLetterList } from "./client"
import { Code } from "./menu"
import { useTabState, Tabs, TabList, Tab, TabPanel } from "../../tab"
import { useTable, Table, pageCount } from "../../table"
import sequenceColumn from "../../table/column/sequence"
import dateTimeColumn from "../../table/column/dateTime"
import { Trans } from "../../i18n"
import {
  from as statusFrom,
  Label as StatusLabel,
  InVerify,
  InVerifyName,
  Published,
  PublishedName,
  Rejected,
  RejectedName
} from "./status"
import { AddButton, VerifyButton, ViewButton } from "./edit"
import { Verify as VerifyMenu, View as ViewMenu } from "./menu"

export { Code }

export const Content = () => {
  let isDisplay = useIsDisplayMenuByRoute()
  if (!isDisplay) {
    return null
  }

  return null
  return <Body />
}

const Body = () => {
  let hasVerifyAuth = useIsDisplayMenuByRoute([VerifyMenu])
  let hasViewAuth = useIsDisplayMenuByRoute([ViewMenu])
  let [statusName, setStatusName] = useState(
    hasVerifyAuth ? InVerifyName : hasViewAuth ? PublishedName : null
  )
  let tab = useTabState({
    selectedId: statusName
  })

  let { state, setState, setPageIndex, setSortOrder } = useTable()
  let { pageIndex, pageSize, sortby, orderby } = state
  let { data = {}, isLoading, isValidating, mutate } = useStationLetterList({
    page: pageIndex + 1,
    rows: pageSize,
    sortby,
    orderby,
    status: statusFrom(statusName)
  })

  let columns = useColumns({ mutate, statusName })

  const setStatus = (newStatusName) => () => {
    if (newStatusName === statusName) {
      mutate()
    }
    setStatusName(newStatusName)
  }

  return (
    <PageContent
      title={<Trans zh="站内信" en="Station Letter" />}
      actionBar={<ActionBar mutate={mutate} />}
    >
      <Tabs>
        <TabList {...tab}>
          {hasVerifyAuth && (
            <Tab
              tabId={InVerifyName}
              onClick={setStatus(InVerifyName)}
              {...tab}
            >
              <StatusLabel value={InVerify} />
            </Tab>
          )}
          <Tab
            tabId={PublishedName}
            onClick={setStatus(PublishedName)}
            {...tab}
          >
            <StatusLabel value={Published} />
          </Tab>
          <Tab tabId={RejectedName} onClick={setStatus(RejectedName)} {...tab}>
            <StatusLabel value={Rejected} />
          </Tab>
        </TabList>
        {hasVerifyAuth && (
          <TabPanel tabId={InVerifyName} {...tab}>
            <Table
              columns={columns}
              data={data?.items}
              pageCount={pageCount(pageSize, data?.totalCount)}
              setPageIndex={setPageIndex}
              setSortOrder={setSortOrder}
              initialState={state}
              isLoading={isLoading}
            />
          </TabPanel>
        )}

        <TabPanel tabId={PublishedName} {...tab}>
          <Table
            columns={columns}
            data={data?.items}
            pageCount={pageCount(pageSize, data?.totalCount)}
            setPageIndex={setPageIndex}
            setSortOrder={setSortOrder}
            initialState={state}
            isLoading={isLoading}
          />
        </TabPanel>

        <TabPanel tabId={RejectedName} {...tab}>
          <Table
            columns={columns}
            data={data?.items}
            pageCount={pageCount(pageSize, data?.totalCount)}
            setPageIndex={setPageIndex}
            setSortOrder={setSortOrder}
            initialState={state}
            isLoading={isLoading}
          />
        </TabPanel>
      </Tabs>
    </PageContent>
  )
}

const ActionBar = ({ mutate }) => {
  return (
    <Box alignX="right" marginBottom="-1rem">
      <AddButton onSubmitted={mutate} />
    </Box>
  )
}

const useColumns = ({ mutate, statusName }) => {
  let columns = [
    sequenceColumn(),
    {
      accessor: "subject",
      Header: <Trans zh="主题" en="Subject" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      }
    },
    {
      accessor: "createdByUsername",
      Header: <Trans zh="操作人" en="Operator" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      }
    },
    dateTimeColumn({
      accessor: "createdOn",
      Header: <Trans zh="创建时间" en="Created On" />,
      disableSortBy: true
    }),
    {
      id: "status",
      accessor: (record) => record,
      Header: <Trans zh="状态" en="Status" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value }) => {
        let { status } = value
        return (
          <Text.Block
            css={css`
              white-space: nowrap;
            `}
          >
            <StatusLabel value={status} />
          </Text.Block>
        )
      }
    }
  ]

  let hasVerifyAuth = useIsDisplayMenuByRoute([VerifyMenu])
  let hasViewAuth = useIsDisplayMenuByRoute([ViewMenu])
  let hasAnyMenu = statusName === InVerifyName ? hasVerifyAuth : hasViewAuth

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
              {statusName === InVerifyName && (
                <VerifyButton record={value} onSubmitted={mutate} />
              )}
              {(statusName === PublishedName ||
                statusName === RejectedName) && <ViewButton record={value} />}
            </Set>
          </Box>
        )
      }
    })
  }

  return useMemo(() => columns, [statusName])
}
