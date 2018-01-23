require 'green_shoes'

def refresh_list
  @points = Array.new
  info = `sudo iwlist scan`.split("Cell")
  info.delete_at(0)
  info.each do |item|
    name = item.scan(/ESSID:.+/).first.sub("ESSID:", "").gsub(/"/, "")
    address = item.scan(/Address:.+/).first
    signal = item.scan(/Quality=[\w]+\/[\w]+/).first
    aut = item.scan(/IE: [\/\.\w\s]+\n/).first
    point = "#{name}   #{address}   #{signal}   #{aut}"
    @points.insert(-1, point)
  end
  return @points
end

Shoes.app title: 'WI-FI', width: 600, height: 200 do
   para 'Choose wi-fi point'
   @list_box = list_box items: refresh_list, width: 500
   button "Refresh list" do
     @list_box.items = refresh_list
   end
   button "Connect" do
     `nmcli dev wifi connect #{ @list_box.text.scan(/[\w\W]+Address/).first.chomp(" Address")}`
   end

   #обновление списка
   Thread.new do
     loop do
       @list_box.items = refresh_list
       sleep 5
     end
   end
end
