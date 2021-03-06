# encoding: ascii-8bit
# 
# Licensed to the Apache Software Foundation (ASF) under one
# or more contributor license agreements. See the NOTICE file
# distributed with this work for additional information
# regarding copyright ownership. The ASF licenses this file
# to you under the Apache License, Version 2.0 (the
# "License"); you may not use this file except in compliance
# with the License. You may obtain a copy of the License at
# 
#   http://www.apache.org/licenses/LICENSE-2.0
# 
# Unless required by applicable law or agreed to in writing,
# software distributed under the License is distributed on an
# "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
# KIND, either express or implied. See the License for the
# specific language governing permissions and limitations
# under the License.
#

module Thrift
  class MemoryBufferTransport < BaseTransport
    GARBAGE_BUFFER_SIZE = 4*(2**10) # 4kB

    # If you pass a string to this, you should #dup that string
    # unless you want it to be modified by #read and #write
    #--
    # this behavior is no longer required. If you wish to change it
    # go ahead, just make sure the specs pass
    def initialize(buffer = nil)
      @buf = buffer || ''
      @index = 0
    end

    def open?
      return true
    end

    def open
    end

    def close
    end

    def peek
      @index < @buf.size
    end

    # this method does not use the passed object directly but copies it
    def reset_buffer(new_buf = '')
      @buf.replace new_buf
      @index = 0
    end

    def available
      @buf.length - @index
    end

    def read(len)
      data = @buf.slice(@index, len)
      @index += len
      @index = @buf.size if @index > @buf.size
      if @index >= GARBAGE_BUFFER_SIZE
        @buf = @buf.slice(@index..-1)
        @index = 0
      end
      if data.size < len
        raise EOFError, "Not enough bytes remain in buffer"
      end
      data
    end

    def write(wbuf)
      @buf << wbuf
    end

    def flush
    end

    def inspect_buffer
      out = []
      for idx in 0...(@buf.size)
        # if idx != 0
        #   out << " "
        # end
      
        if idx == @index
          out << ">"
        end
      
        out << @buf[idx].to_s(16)
      end
      out.join(" ")
    end
  end
end