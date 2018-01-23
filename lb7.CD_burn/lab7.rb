require 'green_shoes'

Shoes.app title: 'CD/DVD', width: 520, height: 300 do
  `mkdir files`
  para 'Input folder', margin: 10
  @add_buttom = button 'Add files', margin: 10
  @burned = button 'Burned', margin: 10
  @write_list = edit_box width: 520, height: 207
  Thread.new do
    loop do
      Dir.chdir('files')
      @write_list.text = `ls`
      Dir.chdir('../')
      sleep 1
    end
  end
  @add_buttom.click do
    now = `pwd`
    filename = ask_open_file
    `cp "#{filename}" "#{now.delete("\n")}/files"`
  end
  @burned.click do
    `mkisofs -V "volume_ID" -D -l -L -N -J -R -v -o cdrom.iso ~/interfaces-and-peripheral-devices/lb7.CD_burn/files/`
    `umount /dev/cdrom`
    `cdrecord -dev=/dev/cdrom -speed=16 -eject -v cdrom.iso`
    `cdrecord -dev=/dev/cdrom -v blank=fast`
    `rm -Rf /home/artem/interfaces-and-peripheral-devices/lb7.CD_burn/files`
    `mkdir files`
    alert('Done!')
  end
end
